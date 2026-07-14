# main.py
from fastapi import FastAPI
from pydantic import BaseModel
import requests, os, json
from typing import List
from azure.identity import DefaultAzureCredential
from azure.keyvault.secrets import SecretClient
from dotenv import load_dotenv

load_dotenv()  # still useful for local dev fallback

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

GROQ_API_KEY = load_secret("GroqApiKey", fallback_env="GROQ_API_KEY")
GROQ_URL = "https://api.groq.com/openai/v1/chat/completions"


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
        content_blocks.append({
            "type": "image_url",
            "image_url": {"url": url}
        })

    payload = {
        "model": "meta-llama/llama-4-scout-17b-16e-instruct",
        "messages": [{"role": "user", "content": content_blocks}],
        "temperature": 0.2,
        "max_completion_tokens": 400
    }
    headers = {"Authorization": f"Bearer {GROQ_API_KEY}", "Content-Type": "application/json"}
    resp = requests.post(GROQ_URL, headers=headers, json=payload, timeout=30)

    if not resp.ok:
        print("GROQ ERROR BODY:", resp.text)

    resp.raise_for_status()
    content = resp.json()["choices"][0]["message"]["content"]

    content = content.strip().strip("```json").strip("```").strip()
    try:
        result = json.loads(content)
    except json.JSONDecodeError:
        result = {"isValid": False, "confidence": 0, "issues": ["AI response parse error"], "recommendation": "REJECT"}
    return result


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
        content_blocks.append({
            "type": "image_url",
            "image_url": {"url": url}
        })

    payload = {
        "model": "meta-llama/llama-4-scout-17b-16e-instruct",
        "messages": [{"role": "user", "content": content_blocks}],
        "temperature": 0.2,
        "max_completion_tokens": 400
    }
    headers = {"Authorization": f"Bearer {GROQ_API_KEY}", "Content-Type": "application/json"}
    resp = requests.post(GROQ_URL, headers=headers, json=payload, timeout=30)

    if not resp.ok:
        print("GROQ ERROR BODY:", resp.text)

    resp.raise_for_status()
    content = resp.json()["choices"][0]["message"]["content"]

    content = content.strip().strip("```json").strip("```").strip()
    try:
        result = json.loads(content)
    except json.JSONDecodeError:
        result = {"isValid": False, "confidence": 0, "issues": ["AI response parse error"], "recommendation": "REJECT"}
    return result


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)