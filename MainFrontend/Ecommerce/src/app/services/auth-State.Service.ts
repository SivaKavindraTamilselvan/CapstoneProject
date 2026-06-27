import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { ADMIN_ROLES } from "../constant/admin.role.constant";
import { VENDOR_ROLES } from "../constant/vendor.role.constant";

@Injectable({
    providedIn: "root"
})
export class AuthStateService {
    private userIdSubject = new BehaviorSubject<string | undefined>(undefined);
    private userNameSubject = new BehaviorSubject<string | undefined>(undefined);
    private roleIdSubject = new BehaviorSubject<string | undefined>(undefined);
    private adminRoleIdSubject = new BehaviorSubject<string | undefined>(undefined);
    private vendorRoleIdSubject = new BehaviorSubject<string | undefined>(undefined);
    private adminRoleNameSubject = new BehaviorSubject<string | undefined>(undefined);
    private vendorRoleNameSubject = new BehaviorSubject<string | undefined>(undefined);

    userId = this.userIdSubject.asObservable();
    roleId = this.roleIdSubject.asObservable();
    adminRoleId = this.adminRoleIdSubject.asObservable();
    vendorRoleId = this.vendorRoleIdSubject.asObservable();
    userName = this.userNameSubject.asObservable();
    adminRoleName = this.adminRoleNameSubject.asObservable();
    vendorRoleName = this.vendorRoleNameSubject.asObservable();

    constructor() {
        this.loadUserToken();
    }
    private getPayLoad(): any {
        const token = sessionStorage.getItem("token");
        if (!token) {
            return null;
        }
        try {
            return JSON.parse(atob(token.split(".")[1]));

        }
        catch {
            return null;
        }
    }
    private loadUserToken() {
        const payload = this.getPayLoad();
        const adminRoleId = payload?.["AdminRoleId"];
        const vendorRoleId = payload?.["VendorRoleId"];
        this.adminRoleIdSubject.next(payload?.["AdminRoleId"]);
        this.adminRoleNameSubject.next(adminRoleId ? ADMIN_ROLES[adminRoleId] : undefined);
        this.vendorRoleIdSubject.next(payload?.["VendorRoleId"]);
        this.vendorRoleNameSubject.next(this.vendorRoleId ? VENDOR_ROLES[vendorRoleId] : undefined);
        this.roleIdSubject.next(payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
        this.userNameSubject.next(payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
        this.userIdSubject.next(payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]);
    }

    login(token: string) {
        sessionStorage.setItem("token", token);
        this.loadUserToken();
    }

    logout() {
        sessionStorage.removeItem("token");
        this.userIdSubject.next(undefined);
        this.roleIdSubject.next(undefined);
        this.adminRoleIdSubject.next(undefined);
        this.vendorRoleIdSubject.next(undefined);
        this.adminRoleNameSubject.next(undefined);
        this.userNameSubject.next(undefined);
    }

    isLoggedIn(): boolean {
        return sessionStorage.getItem("token") !== null;
    }
    getAdminRole(): string | undefined {
        return this.adminRoleNameSubject.getValue();
    }
    getVendorRole(): string | undefined {
        return this.vendorRoleNameSubject.getValue();
    }
    getRoleId(): string | undefined {
        return this.roleIdSubject.getValue();
    }
}