# AI Product Validation Module

## Concept Overview
- A cross-language feature: a C# `AiProductValidationService` (in the main .NET backend) calls out to a separate Python **FastAPI** microservice, which in turn calls the Anthropic Messages API (Claude Sonnet) to QA-check product/variant listings before they go live.
- Purpose: catch mismatches between what a vendor *says* a product is (name, description, category, attributes) and what its images actually show, plus catch internally inconsistent or gibberish listing data.
- Two parallel flows exist — one for base **products** (admin-facing) and one for **product variants** (vendor-facing) — sharing the same image-fetch, prompt-building, and JSON-parsing machinery.

## C# Side — `AiProductValidationService`
- Two entry points:
  - `ValidateAsync(ResponseAdminGetAllProductDTO product)` — validates a base product
  - `ValidateVariantAsync(ResponseVendorGetProductVariantOnly variant)` — validates a specific variant (adds SKU + attributes to the payload)
- **Image handling:** for each product/variant image, resolves the image URL (absolute URLs used as-is; relative URLs prefixed with the configured frontend base URL), fetches the bytes via a plain `HttpClient` (`_imageClient`, from `IHttpClientFactory` — separate from the AI-service client), and converts to a `data:<mime>;base64,<...>` URI. If a MIME type isn't returned by the server, it's guessed from the file extension (`GuessMimeTypeFromExtension`).
- Failed image fetches are swallowed (`catch { return null; }`) and simply excluded from the payload rather than failing the whole validation.
- Builds a JSON payload (camelCase, via a shared `JsonSerializerOptions`) containing the product/variant fields plus the list of base64 image data URIs, and POSTs it to the AI microservice (`_httpClient`, whose `BaseAddress` is configured centrally from Key Vault — not hardcoded).
- Non-success responses throw `HttpRequestException` with the status code and response body included.
- Successful responses are deserialized into `AiValidationResult` (`IsValid`, `Confidence`, `Issues`, `Recommendation`).

## Python Side — FastAPI Microservice (`main.py`)
- Loads its Anthropic API key and base URL either from **Azure Key Vault** (via `DefaultAzureCredential` + `SecretClient`, when `KeyVaultName` is set) or from local `.env`/environment variables as a fallback — same Key Vault pattern used elsewhere in the platform.
- Talks to Claude Sonnet (`claude-sonnet-4-6`) directly via the raw `/v1/messages` REST endpoint using `requests`, not the Python SDK.
- **Image source handling (`image_url_to_base64_block`)** accepts either:
  - An already-encoded `data:` URI (from the C# side) — parsed directly, no network call
  - A real HTTP(S) URL — downloaded and base64-encoded on the Python side
  - This dual path exists because the Presidio LLM gateway proxy currently in front of this service does **not** support Anthropic's `"url"` image source type and requires all images sent as base64 data.
- Two endpoints, mirroring the C# side:
  - `POST /validate-product` — takes name/description/category/subCategory/imageUrls
  - `POST /validate-variant` — takes productName/description/category/subCategory/sku/attributes/imageUrls
- Each endpoint builds a strict QA prompt (`build_prompt` / `build_variant_prompt`) instructing Claude to check image-vs-data consistency, flag unrelated images, and validate that category/subcategory logically fit — then requests a **strict JSON-only** response.
- `call_claude` sends the prompt + image blocks to the Anthropic API and concatenates all `text`-type content blocks from the response.
- `parse_json_response` strips any stray ```` ```json ```` code fences before parsing; if parsing still fails, it returns a safe default (`isValid: false`, `confidence: 0`, `"AI response parse error"`, `recommendation: "REJECT"`) rather than throwing — so a malformed model response fails closed, not silently open.

## Variant-Specific Validation Rules
- Variant images are checked against **visually verifiable** attributes only (e.g. `Color: Red` should visually show red); attributes that can't be visually confirmed (e.g. `Material: Cotton`) are explicitly excluded from penalization in the prompt instructions.
- SKU, name, and description are checked for being non-misleading and non-gibberish, same as the base product flow.

## Edge Cases Handled
- Missing/blank image URLs are skipped before ever reaching the HTTP fetch step
- Individual image download failures don't fail the whole validation — that image is just omitted from what Claude sees
- Missing MIME type headers are worked around via extension guessing (C#) / `mimetypes.guess_type` (Python)
- The C# service's AI-service `HttpClient` is configured centrally (Key Vault-driven `BaseAddress`) rather than hardcoded, so environment (local vs Azure) doesn't require code changes
- Non-2xx responses from the AI microservice are surfaced with both status code and body for debugging, instead of a generic failure
- Malformed/non-JSON model output is caught and converted into a conservative "reject" result instead of crashing the request