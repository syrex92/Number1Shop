import { Routes, Route, Navigate } from 'react-router-dom';
import './App.css';
import { useStores } from './context/RootStoreContext';
import LoginPage from './pages/LoginPage';
import MainLayout from './pages/MainLayout';

function App() {
  const { auth } = useStores();

  return (
    <Routes>
      <Route path="/login" element={auth.isAuthenticated ? <Navigate to="/" replace /> : <LoginPage />} />
      <Route path="/*" element={<MainLayout />} />
    </Routes>
  );
}

export default App;
