import {observer} from 'mobx-react-lite';
import {useStores} from '../context/RootStoreContext';
import ProductCard from '../components/ProductCard';
import '../styles/ProductsPage.css';
import {useEffect} from "react";
import {Center, Loader, Stack, Text} from "@mantine/core";

const ProductsPage = observer(() => {
    const {products, favorites} = useStores();

    useEffect(() => {
        products.fetchProducts();
    }, []);
    
    return (
        <>
            {products.isLoading ?
                <Center>
                    <Stack justify="center" align="center">
                        <Loader size={64}/>
                        <Text>Загрузка каталога...</Text>
                    </Stack>
                </Center>

                : (
                    <div className="products-grid">
                        {products.filteredProducts.map(p => (
                            <ProductCard key={p.id} product={p} isFavorite={favorites.isFavorite(p.id)}/>
                        ))}
                    </div>
                )}
        </>
    );
});

export default ProductsPage;


