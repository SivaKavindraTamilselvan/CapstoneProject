# main.py
from fastapi import FastAPI
from pydantic import BaseModel
import requests, os, json, base64, mimetypes
from typing import List
from azure.identity import DefaultAzureCredential
from azure.keyvault.secrets import SecretClient
from dotenv import load_dotenv

load_dotenv() 

app = FastAPI()

KEY_VAULT_NAME = os.getenv("KeyVaultName")


def load_secret(name: str, fallback_env: str = None) -> str:
    if KEY_VAULT_NAME:
        vault_url = f"https://{KEY_VAULT_NAME}.vault.azure.net"
        credential = DefaultAzureCredential()
        client = SecretClient(vault_url=vault_url, credential=credential)
        return client.get_secret(name).value
    if fallback_env:
        return os.getenv(fallback_env, "")
    return ""


ANTHROPIC_API_KEY = load_secret("AnthropicApiKey", fallback_env="ANTHROPIC_API_KEY")
ANTHROPIC_BASE_URL = load_secret("AnthropicBaseUrl", fallback_env="ANTHROPIC_BASE_URL") or "https://api.anthropic.com"
ANTHROPIC_URL = f"{ANTHROPIC_BASE_URL.rstrip('/')}/v1/messages"
ANTHROPIC_VERSION = "2023-06-01"
MODEL_NAME = "claude-sonnet-4-6"


def image_url_to_base64_block(url: str) -> dict:
    """
    Returns an Anthropic base64 image content block from either:
      1. A real HTTP(S) image URL -- downloaded and base64-encoded here, or
      2. An already-encoded data URI (e.g. "data:image/jpeg;base64,...") -- parsed directly,
         no network request needed.

    NOTE: the Presidio LLM gateway proxy this service is currently pointed at does not support
    the "url" image source type ("URL sources are not supported" error) -- it requires images to
    be sent as base64 data, one way or another.
    """
    if url.startswith("data:"):
        try:
            header, encoded = url.split(",", 1)
            media_type = header.split(":", 1)[1].split(";")[0] or "image/jpeg"
        except (IndexError, ValueError):
            media_type = "image/jpeg"
            encoded = url.split(",", 1)[-1]

        return {
            "type": "image",
            "source": {
                "type": "base64",
                "media_type": media_type,
                "data": encoded,
            },
        }

    resp = requests.get(url, timeout=15)
    resp.raise_for_status()

    content_type = resp.headers.get("Content-Type")
    if not content_type or not content_type.startswith("image/"):
        guessed, _ = mimetypes.guess_type(url)
        content_type = guessed or "image/jpeg"

    encoded = base64.b64encode(resp.content).decode("utf-8")

    return {
        "type": "image",
        "source": {
            "type": "base64",
            "media_type": content_type,
            "data": encoded,
        },
    }


def call_claude(content_blocks: list) -> str:
    """Calls the Anthropic Messages API (or configured gateway) and returns the reply text."""
    payload = {
        "model": MODEL_NAME,
        "max_tokens": 400,
        "temperature": 0.2,
        "messages": [
            {"role": "user", "content": content_blocks}
        ],
    }
    headers = {
        "x-api-key": ANTHROPIC_API_KEY,
        "anthropic-version": ANTHROPIC_VERSION,
        "Content-Type": "application/json",
    }
    resp = requests.post(ANTHROPIC_URL, headers=headers, json=payload, timeout=60)

    if not resp.ok:
        print("ANTHROPIC ERROR BODY:", resp.text)

    resp.raise_for_status()
    data = resp.json()

    text_parts = [block["text"] for block in data.get("content", []) if block.get("type") == "text"]
    return "".join(text_parts)


def parse_json_response(content: str) -> dict:
    content = content.strip().strip("```json").strip("```").strip()
    try:
        return json.loads(content)
    except json.JSONDecodeError:
        return {
            "isValid": False,
            "confidence": 0,
            "issues": ["AI response parse error"],
            "recommendation": "REJECT",
        }


class ProductValidationRequest(BaseModel):
    name: str
    description: str
    category: str
    subCategory: str
    imageUrls: List[str]


def build_prompt(p: ProductValidationRequest):
    return f"""
You are a strict e-commerce product QA reviewer.
You will be shown one or more images of the same product.
Check if the images collectively match the product data below, and if the data itself is consistent.

Product Name: {p.name}
Description: {p.description}
Category: {p.category}
Subcategory: {p.subCategory}

Rules:
1. All images must visually match the product name and description (different angles are fine).
2. If any image looks unrelated to the others or to the product data, flag it.
3. Category and Subcategory must logically fit the product.
4. Name/description must not be misleading, empty, or gibberish.

Respond ONLY in strict JSON:
{{
  "isValid": true/false,
  "confidence": 0-100,
  "issues": ["short reason 1", "short reason 2"],
  "recommendation": "ACCEPT" or "REJECT"
}}
"""


@app.post("/validate-product")
def validate_product(p: ProductValidationRequest):
    content_blocks = [{"type": "text", "text": build_prompt(p)}]

    for url in p.imageUrls:
        content_blocks.append(image_url_to_base64_block(url))

    content = call_claude(content_blocks)
    return parse_json_response(content)


class VariantAttribute(BaseModel):
    attributeName: str
    attributeValue: str


class ProductVariantValidationRequest(BaseModel):
    productName: str
    description: str
    productCategoryName: str
    productSubCategoryName: str
    sku: str
    attributes: List[VariantAttribute]
    imageUrls: List[str]


def build_variant_prompt(p: ProductVariantValidationRequest):
    attrs_text = "\n".join(
        f"- {a.attributeName}: {a.attributeValue}" for a in p.attributes
    ) or "No attributes provided"

    return f"""
You are a strict e-commerce product QA reviewer.
You will be shown one or more images of a specific product VARIANT (e.g. a particular color/size combination of a base product).
Check if the images match the variant's declared attributes, and if the data is internally consistent.

Base Product Name: {p.productName}
Description: {p.description}
Category: {p.productCategoryName}
Subcategory: {p.productSubCategoryName}
SKU: {p.sku}

Variant Attributes:
{attrs_text}

Rules:
1. Images must visually match the base product name/description (different angles are fine).
2. Images must visually match the declared variant attributes where visually verifiable — for example, if an attribute is "Color: Red", the product in the image should appear red. If an attribute cannot be visually verified (e.g. "Material: Cotton"), do not penalize for it.
3. If any image looks unrelated to the others or to the product/variant data, flag it.
4. Category and Subcategory must logically fit the product.
5. SKU, name, and description must not be misleading, empty, or gibberish.

Respond ONLY in strict JSON:
{{
  "isValid": true/false,
  "confidence": 0-100,
  "issues": ["short reason 1", "short reason 2"],
  "recommendation": "ACCEPT" or "REJECT"
}}
"""


@app.post("/validate-variant")
def validate_variant(p: ProductVariantValidationRequest):
    content_blocks = [{"type": "text", "text": build_variant_prompt(p)}]

    for url in p.imageUrls:
        content_blocks.append(image_url_to_base64_block(url))

    content = call_claude(content_blocks)
    return parse_json_response(content)


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)