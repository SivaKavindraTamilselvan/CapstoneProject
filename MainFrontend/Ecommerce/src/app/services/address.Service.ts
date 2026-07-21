import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { AddAddressModel } from "../models/address/add-address.model";
import { AddressFilter } from "../models/address/address-filter";
import { PagedResponse } from "../models/paged-response.model";
import { AddressModel } from "../models/address/address-response.model";

@Injectable({
    providedIn: "root"
})
export class AddressService {
    constructor(private http: HttpClient) {

    }
    // for warehouse and the user
    addAddress(request: AddAddressModel) {
        let url = BaseURL + "/Address/add-address";
        return this.http.post(url, request);
    }
    //get address by id
    getAddressId(id: number) {
        let url = BaseURL + `/Address/address/${id}`;
        return this.http.get(url,{});
    }

    // get address for warehouse 
    getAddress(filter: AddressFilter) {
        let url = BaseURL + "/Address/vendor-address";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AddressModel>>(url, { params });
    }

    // get user address
    getUserAddress() {
        let url = BaseURL + "/Address/actice-address";
        return this.http.get<AddressModel[]>(url, {});
    }

    // delete the vendor warehouse address
    deleteAddress(addressId: number) {
        console.log(addressId);
        let url = BaseURL + `/Address/vendor/${addressId}/deactivate`;
        return this.http.patch(url, {});
    }

    // delete user address
    deleteUserAddress(addressId: number) {
        let url = BaseURL + `/Address/${addressId}/deactivate`;
        return this.http.patch(url, {});
    }

    // set as default address
    setDefaultAddress(addressid: number) {
        let url = BaseURL + `/Address/${addressid}/default`;
        return this.http.put(url, {});
    }
}