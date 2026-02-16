
// export interface ProductItemResponseDto {
//     id: string;
//     stockQuantity: number;
//     productTitle: string;
//     productDescription: string;
//     productCategory: string;
//     price: number;
//     imagesUrls: string[];
// }

export interface ProductItemResponseDto {
    id: string;
    stockQuantity: number;
    productTitle: string;
    productDescription: string;
    productCategory: string;
    article: number;
    price: number;
    imageUrl?: string;
}

//export default ProductItemResponseDto
