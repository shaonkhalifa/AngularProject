import { Status } from "../shared/app-constants";
import { OrderItem } from "./order-item";

export interface Order {
    orderID?:number;
    orderDate?:Date;
    deliveryDate?:Date;
    status?:Status;
    customerID?:number;
    orderItems?:OrderItem[];
}
