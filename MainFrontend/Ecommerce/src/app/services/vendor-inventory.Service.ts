import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddInventoryModel } from "../models/inventory/add-inventory.model";
import { BaseURL } from "../environment";

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
}