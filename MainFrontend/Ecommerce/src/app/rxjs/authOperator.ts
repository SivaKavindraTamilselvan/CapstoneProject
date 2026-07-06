import { Subject } from "rxjs";
import { AuthStateService } from "../services/auth-State.Service";
import { inject } from "@angular/core";

export const usernameSubject = new Subject<string | undefined>();

export const isLoggedIn = () => {
    const authState = inject(AuthStateService);
    authState.validateSession();
    const token = sessionStorage.getItem("token");
    return token ? true : false;
}