import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddInventoryModel } from "../models/inventory/add-inventory.model";
import { BaseURL } from "../environment";
import { VendorInventoryFilterModel } from "../models/inventory/inventory.filter";
import { PagedResponse } from "../models/paged-response.model";
import { VendorInventoryModel } from "../models/inventory/inventory.model";
import { UpdateInventoryModel } from "../models/inventory/update-inventory.model";

@Injectable({
    providedIn: "root"
})
export class VendorInventoryService {
    constructor(private http: HttpClient) {

    }
    addInventory(request: AddInventoryModel) {
        let url = BaseURL + "/VendorInventory/add-inventory";
        return this.http.post(url, request);
    }
    getInventory(filter :VendorInventoryFilterModel){
        let url = BaseURL + "/VendorInventory/vendor-inventories";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorInventoryModel>>(url,{params});
    }
    getInventoryDetails(inventoryId : number){
        let url = BaseURL + `/VendorInventory/vendor-inventories/${inventoryId}`;
        return this.http.get<VendorInventoryModel>(url,{});
    }
    deleteInventory(inventoryId : number){
        let url = BaseURL + `/VendorInventory/vendor-inventories/${inventoryId}`;
        return this.http.patch<VendorInventoryModel>(url,{});
    }
    updateInventory(request : UpdateInventoryModel){
        console.log(request);
        let url = BaseURL + "/VendorInventory/update-inventory";
        return this.http.put(url,request);
    }
}