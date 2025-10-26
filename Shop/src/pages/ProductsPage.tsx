import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import ProductCard from '../components/ProductCard';
import '../styles/ProductsPage.css';

const ProductsPage = observer(() => {
  const { products, favorites } = useStores();
  return (
    <div className="products-grid">
      {products.filteredProducts.map(p => (
        <ProductCard key={p.id} product={p} isFavorite={favorites.isFavorite(p.id)} />
      ))}
    </div>
  );
});

export default ProductsPage;


