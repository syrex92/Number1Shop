import {observer} from "mobx-react-lite";
import {useStores} from "../../context/RootStoreContext.tsx";
import type {CartItem} from "../../stores/CartStore.tsx";
import {
    ActionIcon,
    Box,
    Card,
    Checkbox,
    Flex,
    Grid,
    Group,
    Image,
    NumberFormatter,
    Skeleton,
    Spoiler,
    Stack,
    Text,
    Typography
} from "@mantine/core";
import {useEffect, useState} from "react";
import {IconArrowLeft, IconArrowRight, IconHeart, IconMinus, IconPlus, IconTrash} from "@tabler/icons-react";
import type {ProductItemResponseDto} from "../../dto/ProductItemResponseDto.tsx";
import DOMPurify from "dompurify";
import {Carousel} from "@mantine/carousel";
import shopConfig from "../../config/shopConfig.ts";
import {runInAction} from "mobx";
import {BeatLoader} from "react-spinners";
import {openConfirmModal} from "@mantine/modals";

export interface CartItemCardProps {
    cartItem: CartItem;
}

const CartItemCard = observer(({cartItem}: CartItemCardProps) => {

    const {cart} = useStores();
    const {catalogApiUrl, imagesUrl} = shopConfig;

    //const [product, setProduct] = React.useState<ProductItemResponseDto | undefined>();

    function openDeleteDialog(productId: string) {
        console.log("openDeleteDialog");
        openConfirmModal({
            title: 'Подтвердите удаление',
            children: (
                <Text size="sm">
                    Удалить товар из корзины?
                </Text>
            ),
            labels: {confirm: 'Удалить', cancel: 'Отмена'},
            confirmProps: {color: 'red'},
            onConfirm: async () => {
                setExec(true)
                try {
                    await cart.remove(productId, true)
                } finally {
                    setExec(false)
                }
            },
            onCancel: () => {
            },
        });
    }
    
    const getProduct = () => {

        //products.products

        fetch(`${catalogApiUrl}products/${cartItem.product.id}`)
            .then(res => {
                console.log(res);
                if (res.ok) {
                    return res.json();
                }
            })
            .then(res => res as ProductItemResponseDto)
            .then(data => {
                //setProduct(data);
                runInAction(() => {
                    cartItem.product = {
                        article: data.article,
                        price: data.price,
                        imageUrl: data.imageUrl,
                        category: data.productCategory,
                        description: data.productDescription,
                        title: data.productTitle,
                        id: data.id
                    };
                })

            })
            .catch(err => {
                console.log(err);
            })
        ;
    }

    useEffect(() => {
        if (!cartItem.product) {
            getProduct();
        }
    }, [])

    const productImages = () => {

        if (!cartItem.product || !cartItem.product.imageUrl)
            return (<Text mx={"auto"}>No image</Text>);

        const getSingleImage = () => {
            return (
                <Box h={180} style={{alignContent: "center"}}>
                    <Image radius={10} src={`${imagesUrl}${cartItem.product.imageUrl}`}/>
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
                        //product.imagesUrls.map((url) => (
                        //<Carousel.Slide key={url} style={{alignContent: "center"}}>
                        <Image radius={10} src={`${imagesUrl}${cartItem.product.imageUrl}`}/>
                        //</Carousel.Slide>
                        //))
                    }
                </Carousel>
            );
        }

        return (
            <>
                {
                    cartItem.product && cartItem.product.imageUrl
                        ? getSingleImage()
                        : getSlides()
                }
            </>
        )
    }

    const [exec, setExec] = useState<boolean>(false);

    return (
        <>
            {cartItem.product === undefined ?
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
                                runInAction(() => {
                                    //cartItem.toOrder = event.target.checked
                                    cart.toOrder(cartItem.product.id, event.target.checked);
                                })
                                //cart.toOrder(cartItem.product.id, event.target.checked);
                            }}/>
                        </Grid.Col>
                        <Grid.Col span={2}>
                            {productImages()}
                        </Grid.Col>
                        <Grid.Col span={6}>
                            <Flex justify={"space-evenly"} align={"flex-start"} direction={"column"}>
                                <Text size={"xl"} mb={"md"}>{cartItem.product.title}</Text>
                                <Box size={"md"}>
                                    <Spoiler maxHeight={120} showLabel={"показать больше"} hideLabel={"скрыть"}
                                             style={{textAlign: "justify"}}>
                                        <Typography>
                                            <span
                                                dangerouslySetInnerHTML={{__html: DOMPurify.sanitize(cartItem.product.description ?? 'Описание отсутствует')}}
                                            />
                                        </Typography>
                                    </Spoiler>
                                </Box>
                                <Group m={"xs"} align={"flex-start"}>
                                    <ActionIcon variant="outline" aria-label="Settings" disabled={exec}
                                                onClick={() => console.log("Set favourite")}>
                                        <IconHeart style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                    </ActionIcon>

                                    <ActionIcon variant="outline" aria-label="Settings" disabled={exec}
                                                onClick={async () => {
                                                    setExec(true)
                                                    try {
                                                        openDeleteDialog(cartItem.product.id)
                                                    }
                                                    finally {
                                                        setExec(false)
                                                    }                                                    
                                                }}>
                                        <IconTrash style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                    </ActionIcon>
                                </Group>
                            </Flex>

                        </Grid.Col>
                        <Grid.Col span={3}>
                            <Stack align={"center"}>
                                <Group justify={"center"} h={30}>

                                    {exec && <BeatLoader color={"green"} size={5} />}

                                    {!exec &&
                                        (<>
                                            <ActionIcon variant="outline" aria-label="Settings"
                                                        onClick={async () => {
                                                            setExec(true);
                                                            try {
                                                                await cart.remove(cartItem.product.id, false)
                                                            }
                                                            finally {
                                                                setExec(false)
                                                            }                                                            
                                                        }}>
                                                <IconMinus style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                            </ActionIcon>

                                            <Text>{cart.getItemQuantity(cartItem.product)}</Text>

                                            <ActionIcon variant="outline" aria-label="Settings"
                                                        onClick={async () => {

                                                            setExec(true)
                                                            try {
                                                                await cart.add(cartItem.product);                                                                
                                                            }
                                                            finally {
                                                                setExec(false)
                                                            }
                                                            
                                                            
                                                        }}>
                                                <IconPlus style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                            </ActionIcon>
                                        </>)
                                    }


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