import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddInventoryModel } from "../models/inventory/add-inventory.model";
import { BaseURL } from "../environment";
import { VendorInventoryFilterModel } from "../models/inventory/inventory.filter";
import { PagedResponse } from "../models/paged-response.model";
import { VendorInventoryModel } from "../models/inventory/inventory.model";

@Injectable({
    providedIn: "root"
})
export class VendorInventoryService {
    constructor(private http: HttpClient) {

    }
    addInventory(request: AddInventoryModel) {
        let url = BaseURL + "/Inventory/AddInventory";
        return this.http.post(url, request);
    }
    getInventory(filter :VendorInventoryFilterModel){
        let url = BaseURL + "/Inventory/vendor-inventories";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorInventoryModel>>(url,{params});
    }
}