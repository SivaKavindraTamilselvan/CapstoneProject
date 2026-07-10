export interface Column {
  key: string;
  header: string;
  formatter?: (value: any, row?: any) => any;
}