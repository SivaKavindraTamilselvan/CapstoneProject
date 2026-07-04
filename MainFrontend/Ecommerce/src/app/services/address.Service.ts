import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { AddAddressModel } from "../models/address/add-address.model";
import { AddressFilter } from "../models/address/address-filter";
import { PagedResponse } from "../models/paged-response.model";
import { AddressModel } from "../models/address/address-response.model";
import { DeleteAddress } from "../address/delete-address/delete-address";

@Injectable({
    providedIn: "root"
})
export class AddressService {
    constructor(private http: HttpClient) {

    }
    addAddress(request: AddAddressModel) {
        let url = BaseURL + "/Address/add-address";
        return this.http.post(url, request);
    }
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
    getUserAddress() {
        let url = BaseURL + "/Address/actice-address";
        return this.http.get<AddressModel[]>(url, {});
    }
    deleteAddress(addressId: number) {
        console.log(addressId);
        let url = BaseURL + `/Address/vendor/${addressId}/deactivate`;
        return this.http.patch(url, {});
    }
    deleteUserAddress(addressId: number) {
        let url = BaseURL + `/Address/${addressId}/deactivate`;
        return this.http.patch(url, {});
    }
    setDefaultAddress(addressid: number) {
        let url = BaseURL + `/Address/${addressid}/default`;
        return this.http.patch(url, {});
    }
}