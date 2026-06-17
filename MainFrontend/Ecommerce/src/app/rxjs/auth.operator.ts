import { Subject } from "rxjs";

export const username = new Subject<string | undefined>();


export const logout = () => {
    sessionStorage.removeItem("token");
    username.next(undefined);
}
export const isLoggedIn = () => {
    const token = sessionStorage.getItem("token");
    return token ? true : false;
}
