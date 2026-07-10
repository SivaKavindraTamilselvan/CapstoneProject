export interface TableAction<T> {
    label: string;
    color: string;
    action: string;
    visible?: (row: T) => boolean;
}