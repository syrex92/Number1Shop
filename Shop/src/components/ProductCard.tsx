import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import { FiHeart } from 'react-icons/fi';
import type { Product } from '../stores/ProductsStore';
import '../styles/ProductCard.css';
import AddToCartButton from "./Cart/AddToCartButton.tsx";

interface ProductCardProps {
  product: Product;
  isFavorite: boolean;
}

const ProductCard = observer(({ product, isFavorite }: ProductCardProps) => {
  const { favorites } = useStores();

  return (
    <div className="product-card card">
      <div className="image-wrap">
        <img src={product.image} alt={product.title} className="product-image" />
        <button
          className={`favorite ${isFavorite ? 'active' : ''}`}
          onClick={() => favorites.toggle(product.id)}
          aria-label="Добавить в избранное"
          title={isFavorite ? 'Убрать из избранного' : 'В избранное'}
        >
          <FiHeart />
        </button>
      </div>
      <div className="product-info">
        <div className="product-title">{product.title}</div>
        <div className="product-price">{product.price} ₽</div>
      </div>
        
          <AddToCartButton product={product} />
        
    </div>
  );
});

export default ProductCard;