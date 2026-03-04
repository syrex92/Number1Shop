import type {IOrderItemDto} from "./IOrderItemDto.ts";
import type {IAddressDto} from "./IAddressDto.ts";

export interface IOrderDto {
    id: string;
    userId: string;
    createdAt: string;
    deliveryAddress: IAddressDto;
    items: IOrderItemDto[];
    status: string;
    
}