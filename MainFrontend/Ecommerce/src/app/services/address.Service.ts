import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { AddAddressModel } from "../models/address/add-address.model";

@Injectable({
    providedIn: "root"
})
export class AddressService {
    constructor(private http: HttpClient) {

    }
    addAddress(request : AddAddressModel) {
        let url = BaseURL + "/Address/add-address";
        return this.http.post(url, request);
    }
}