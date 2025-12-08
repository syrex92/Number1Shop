import {observer} from "mobx-react-lite";
import {useStores} from "../../context/RootStoreContext.tsx";
import type {CartItem} from "../../stores/CartStore.tsx";
import {
    Checkbox,
    Grid,
    Image,
    Text,
    Flex,
    Group,
    ActionIcon,
    Card,
    NumberFormatter, Stack, Typography, Skeleton, Spoiler, Box
} from "@mantine/core";
import React, {useEffect} from "react";
import {
    IconArrowLeft,
    IconArrowRight,
    IconHeart,
    IconMinus,
    IconPlus,
    IconTrash
} from "@tabler/icons-react";
import type {ProductItemResponseDto} from "../../dto/ProductItemResponseDto.tsx";

export interface CartItemCardProps {
    cartItem: CartItem;
}

import DOMPurify from "dompurify";
import {Carousel} from "@mantine/carousel";

interface HtmlBlockProps {
    html: string;
}

export const SafeHtmlBlock: React.FC<HtmlBlockProps> = ({html}) => {
    const sanitized = DOMPurify.sanitize(html);

    return (
        <div
            dangerouslySetInnerHTML={{__html: sanitized}}
        />
    );
};

const CartItemCard = observer(({cartItem}: CartItemCardProps) => {

    const {cart} = useStores();

    const [product, setProduct] = React.useState<ProductItemResponseDto | undefined>();

    const getProduct = () => {

        fetch("http://localhost/api/v1/catalog/products/" + cartItem.product.id)
            .then(res => {
                console.log(res);
                if (res.ok) {
                    const js = res.json()
                    return js;
                }
            })
            .then(res => res as ProductItemResponseDto)
            .then(data => {
                setProduct(data);
                //return data;
            })
            .catch(err => {
                console.log(err);
            })
        ;
    }

    useEffect(() => {
        //console.log(cartItem)
        getProduct();
    }, [cartItem])


    useEffect(() => {
        console.log(product);

    }, [product])

    const productImages = () => {

        if (!product || !product.imagesUrls || !product.imagesUrls.length)
            return (<Text>No image</Text>);

        const getSingleImage = () => {
            return (
                <Box h={180} style={{alignContent: "center"}}>
                    <Image radius={10} src={`http://localhost/images/${product.imagesUrls[0]}`} />
                </Box>
            );
        }

        const getSlides = () => {
            return (
                <Carousel
                    height={180}
                    nextControlIcon={<IconArrowRight size={16}/>}
                    previousControlIcon={<IconArrowLeft size={16}/>}
                >
                    {
                        product.imagesUrls.map((url) => (
                            <Carousel.Slide key={url} style={{alignContent: "center"}}>
                                <Image radius={10} src={`http://localhost/images/${url}`}/>
                            </Carousel.Slide>
                        ))
                    }
                </Carousel>
            );
        }

        return (

            <>
                {
                    product && product.imagesUrls && product.imagesUrls.length === 1
                        ? getSingleImage()
                        : getSlides()
                }
            </>

        )
    }

    return (
        <>
            {product === undefined ?
                <>

                    <Card shadow={"xs"}>
                        <Flex justify={"stretch"} align={"flex-start"} direction={"row"}>
                            <Skeleton height={50} circle my="lg" mx={"xl"}/>
                            <Group w={"80%"}>
                                <Skeleton height={8} m={6} ml={0} w={"30vw"} radius="xl"/>
                                <Skeleton height={8} mt={6} radius="xl"/>
                                <Skeleton height={8} mt={6} width="70%" radius="xl"/>
                            </Group>
                        </Flex>
                    </Card>

                </>
                :
                <Card shadow="xs" p="md" radius={"sm"}>
                    <Grid>
                        <Grid.Col span={1}>
                            <Checkbox checked={cartItem.toOrder} onChange={(event) => {
                                cart.toOrder(cartItem.product.id, event.currentTarget.checked);
                            }}/>
                        </Grid.Col>
                        <Grid.Col span={2}>
                            {productImages()}

                        </Grid.Col>
                        <Grid.Col span={6}>
                            <Flex justify={"space-evenly"} align={"flex-start"} direction={"column"}>
                                <Text size={"xl"} mb={"md"}>{product.productTitle}</Text>
                                <Text size={"md"}>
                                    <Spoiler maxHeight={120} showLabel={"показать больше"} hideLabel={"скрыть"}
                                             style={{textAlign: "justify"}}>
                                        <Typography>
                                            <div
                                                dangerouslySetInnerHTML={{__html: DOMPurify.sanitize(product.productDescription)}}
                                            />
                                        </Typography>
                                    </Spoiler>
                                </Text>
                                <Group m={"xs"} align={"flex-start"}>
                                    <ActionIcon variant="outline" aria-label="Settings"
                                                onClick={() => console.log("Set favourite")}>
                                        <IconHeart style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                    </ActionIcon>

                                    <ActionIcon variant="outline" aria-label="Settings"
                                                onClick={() => cart.remove(cartItem.product.id)}>
                                        <IconTrash style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                    </ActionIcon>
                                </Group>
                            </Flex>

                        </Grid.Col>
                        <Grid.Col span={3}>
                            <Stack align={"center"}>
                                <Group justify={"center"}>

                                    <ActionIcon variant="outline" aria-label="Settings"
                                                onClick={() => cart.decrease(cartItem.product.id)}>
                                        <IconMinus style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                    </ActionIcon>

                                    <Text>{cartItem.qty}</Text>

                                    <ActionIcon variant="outline" aria-label="Settings"
                                                onClick={() => cart.add(cartItem.product)}>
                                        <IconPlus style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                    </ActionIcon>


                                </Group>
                                <NumberFormatter
                                    thousandSeparator=" "
                                    decimalSeparator=","
                                    value={cartItem.qty * cartItem.product.price}
                                    suffix={" ₽"}
                                    decimalScale={2}
                                />
                            </Stack>
                        </Grid.Col>
                    </Grid>
                </Card>
            }
        </>
    )
})

export default CartItemCard;