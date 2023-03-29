import { Status } from "../shared/app-constants";

export interface OrderViewModel {
    orderID?:number;
    orderDate?:Date;
    deliveryDate?:Date;
    status?:Status;
    customerID?:number;
    customerName?:string;
    orderValue?:number;
}
