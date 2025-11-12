import {Button, Group, Indicator} from "@mantine/core";
import {IconShoppingCart} from "@tabler/icons-react";
import {useStores} from "../../context/RootStoreContext.tsx";
import {observer} from "mobx-react-lite";
import {useNavigate} from "react-router-dom";

const CartLinkButton = observer(() => {

    const {cart} = useStores();
    const navigate = useNavigate();
       
    const onNavigateClick = () => {
        navigate("/cart");
    }

    const isEmpty = cart.count === 0;
    
    return (
        <div>
            <Group justify="center">
                <Indicator size="16" color="orange" label={"" + cart.count} disabled={isEmpty} >
                    <Button
                        onClick={onNavigateClick}
                        rightSection={<IconShoppingCart size={14}/>}
                        variant="default">
                        Корзина
                    </Button>
                </Indicator>
            </Group>          
            
        </div>


    )
})

export default CartLinkButton;