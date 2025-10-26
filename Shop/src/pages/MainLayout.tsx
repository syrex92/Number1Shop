import { useEffect } from 'react';
import { NavLink, Routes, Route, Navigate } from 'react-router-dom';
import { FiBox, FiShoppingCart, FiHeart, FiPackage, FiLogOut, FiLogIn, FiSearch } from 'react-icons/fi';
import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import ProductsPage from './ProductsPage';
import OrdersPage from './OrdersPage';
import CartPage from './CartPage';
import FavoritesPage from './FavoritesPage';
import '../styles/MainLayout.css';

const MainLayout = observer(() => {
  const { auth, products } = useStores();

  useEffect(() => {
    products.fetchProducts();
  }, [products]);

  return (
    <div className="layout">
      <header className="header">
        <div className="logo">Number One Shop</div>
        <div className="search">
          <FiSearch className="search-icon" />
          <input
            placeholder="Поиск товаров"
            value={products.query}
            onChange={(e) => products.setQuery(e.target.value)}
          />
        </div>
        <div className="user">
          {auth.isAuthenticated ? (
            <button className="btn" onClick={auth.logout}><FiLogOut /> Выйти</button>
          ) : (
            <NavLink className="btn" to="/login"><FiLogIn /> Войти</NavLink>
          )}
        </div>
      </header>

      <nav className="tabs">
        <NavLink to="/" end><FiBox /> <span>Товары</span></NavLink>
        <NavLink to="/orders"><FiPackage /> <span>Заказы</span></NavLink>
        <NavLink to="/cart"><FiShoppingCart /> <span>Корзина</span></NavLink>
        <NavLink to="/favorites"><FiHeart /> <span>Избранное</span></NavLink>
      </nav>

      <main className="content">
        <Routes>
          <Route index element={<ProductsPage />} />
          <Route path="orders" element={<OrdersPage />} />
          <Route path="cart" element={<CartPage />} />
          <Route path="favorites" element={<FavoritesPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </div>
  );
});

export default MainLayout;


