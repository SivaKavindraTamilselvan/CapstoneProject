import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { isLoggedIn } from "../rxjs/authOperator";
import { AuthStateService } from "../services/auth-State.Service";

export const roleGuard: CanActivateFn = (route) => {
    const router = inject(Router);
    const authStateService = inject(AuthStateService);
    const userStatus = isLoggedIn();
    if (!userStatus) {
        router.navigate(["login"]);
        return false;
    }
    const role = authStateService.getRoleId();
    //console.log(role);
    const allowedRoles = route.data['roles'] as string[];
    //console.log(allowedRoles);

    if (!role) {
        router.navigate(['/unauthorized']);
        return false;
    }

    if (allowedRoles.includes(role)) {
        return true;
    }
    router.navigate(['/unauthorized']);
    return false;
}