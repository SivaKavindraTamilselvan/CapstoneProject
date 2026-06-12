using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Org.BouncyCastle.Ocsp;

public partial class AdminProductService : IAdminProductService
{

    public async Task<PagedResponse<ResponseAdminGetAllProductDTO>> GetAllProductsForAdmin(RequestAdminProductFilter request)
    {
        var result = await _productRepsository.GetAdminProduct(request);
        var products = result.items;
        var response = _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
        for (int i = 0; i < products.Count; i++)
        {
            var validation = await _productValidation.ValidateProductChain(products[i]);

            response[i].IsAvailableForSale = products[i].ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && products[i].ProductStatusId == (int)ProductStatusEnum.Active
            && validation.IsValid && products[i].ProductVariants.Any(pv => pv.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && pv.ProductVariantStatusId == (int)ProductStatusEnum.Active
            && pv.Inventories.Any(inv => inv.AvailableQuantity > 0));

            response[i].ValidationIssues = validation.Issues;
        }
        if (request.hasIssues.HasValue)
        {
            if (request.hasIssues.Value)
            {
                response = response.Where(p => p.ValidationIssues.Any()).ToList();
            }
            else
            {
                response = response.Where(p => !p.ValidationIssues.Any()).ToList();
            }
        }
        if (request.isAvailableForSale.HasValue)
        {
            response = response.Where(p => p.IsAvailableForSale == request.isAvailableForSale.Value).ToList();
        }
        return new PagedResponse<ResponseAdminGetAllProductDTO>
        {
            Items = response,
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<PagedResponse<ResponseGetAllProductVariant>> GetAllProductVariant(RequestAdminProductVariantFilter request)
    {
        var result = await _productVariantRepsository.GetAllVariantsForAdmin(request);
        var products = result.Items;
        var response = _mapper.Map<List<ResponseGetAllProductVariant>>(products);
        for (int i = 0; i < products.Count; i++)
        {
            var validation = await _productValidation.ValidateProductChain(products[i].Product!);

            response[i].IsAvailableForSale = products[i].ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && products[i].ProductVariantStatusId == (int)ProductStatusEnum.Active
            && validation.IsValid && products[i].Inventories.Any(inv => inv.AvailableQuantity > 0);

            response[i].ValidationIssues = validation.Issues;
        }
        if (request.hasIssues.HasValue)
        {
            if (request.hasIssues.Value)
            {
                response = response.Where(p => p.ValidationIssues.Any()).ToList();
            }
            else
            {
                response = response.Where(p => !p.ValidationIssues.Any()).ToList();
            }
        }
        if (request.isAvailableForSale.HasValue)
        {
            response = response.Where(p => p.IsAvailableForSale == request.isAvailableForSale.Value).ToList();
        }
        return new PagedResponse<ResponseGetAllProductVariant>
        {
            Items = _mapper.Map<List<ResponseGetAllProductVariant>>(products),
            TotalCount = result.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId)
    {
        var product = await _productRepsository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            throw new DataNotFoundException("Product not found");
        }
        return _mapper.Map<ResponseAdminGetAllProductDTO>(product);
    }
}