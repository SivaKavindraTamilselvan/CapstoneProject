import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ShipmentFilter } from "../models/admin/admin-shipment/shipment.filter";
import { BaseURL } from "../environment";
import { ShipmentModel } from "../models/admin/admin-shipment/admin-shipment.model";
import { PagedResponse } from "../models/paged-response.model";
import { UpdateShipment } from "../admin/admin-shipment/update-shipment/update-shipment";
import { ShipmentUpdateModel } from "../models/admin/admin-shipment/update-shipment.model";
import { ShipmentUpdateRequestModel } from "../models/admin/admin-shipment/update-shipment.dto";

@Injectable({
    providedIn: "root"
})
export class AdminShipmentService {
    constructor(private http: HttpClient) {

    }
    getShipment(filter: ShipmentFilter) {
        let url = BaseURL + "/Shiprocket";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<ShipmentModel>>(url, { params });
    }
   
    updateShipment(request : ShipmentUpdateRequestModel){
        let url = BaseURL + "/Shiprocket/updateShipmentStatus";
         return this.http.put(url,request);
    }
    getShipmentDetails(id: number) {
        let url = BaseURL + `/Shiprocket/${id}`;
        return this.http.get(url, {});
    }
}