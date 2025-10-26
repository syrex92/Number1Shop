import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import { FiHeart, FiShoppingCart } from 'react-icons/fi';
import type { Product } from '../stores/ProductsStore';
import '../styles/ProductCard.css';

interface ProductCardProps {
  product: Product;
  isFavorite: boolean;
}

const ProductCard = observer(({ product, isFavorite }: ProductCardProps) => {
  const { cart, favorites } = useStores();

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
      <button className="add-to-cart btn" onClick={() => cart.add(product)}>
        <FiShoppingCart /> В корзину
      </button>
    </div>
  );
});

export default ProductCard;