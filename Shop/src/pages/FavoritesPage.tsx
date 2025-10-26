import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import ProductCard from '../components/ProductCard';
import '../styles/FavoritesPage.css';

const FavoritesPage = observer(() => {
  const { products, favorites } = useStores();
  const favList = products.products.filter(p => favorites.isFavorite(p.id));
  return (
    <div className="favorites-grid">
      {favList.length === 0 ? (
        <div className="empty">Избранных товаров пока нет</div>
      ) : (
        favList.map(p => (
          <ProductCard key={p.id} product={p} isFavorite={true} />
        ))
      )}
    </div>
  );
});

export default FavoritesPage;


