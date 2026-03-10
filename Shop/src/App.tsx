import { Routes, Route, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import './App.css';
import LoginPage from './pages/LoginPage';
import MainLayout from './pages/MainLayout';
import '@mantine/core/styles.css';
import '@mantine/carousel/styles.css';
import '@mantine/notifications/styles.css';
import { MantineProvider } from '@mantine/core';
import RegistrationPage from './pages/RegistrationPage';
import { ModalsProvider } from "@mantine/modals";
import { Notifications } from "@mantine/notifications";
import { useStores } from './context/RootStoreContext';


function App() {
    const [initialized, setInitialized] = useState(false);
    const { auth } = useStores();
    const navigate = useNavigate();

    useEffect(() => {
        // Просто проверяем, есть ли сохраненный пользователь
        const checkAuth = async () => {
            const isAuth = await auth.checkAuth();
            if (isAuth && window.location.pathname === '/login') {
                navigate('/');
            }
            setInitialized(true);
        };
        
        checkAuth();
    }, [auth, navigate]);

    if (!initialized) {
        return (
            <MantineProvider defaultColorScheme="dark">
                <div style={{ 
                    display: 'flex', 
                    justifyContent: 'center', 
                    alignItems: 'center', 
                    height: '100vh' 
                }}>
                    Загрузка...
                </div>
            </MantineProvider>
        );
    }

    return (
        <MantineProvider defaultColorScheme="dark">
            <Notifications />
            <ModalsProvider>
                <Routes>
                    <Route path="/login" element={<LoginPage/>}/>
                    <Route path="/registration" element={<RegistrationPage/>}/>
                    <Route path="/" element={<MainLayout/>}/>
                    <Route path="*" element={<MainLayout/>}/>
                </Routes>
            </ModalsProvider>
        </MantineProvider>
    );
}

export default App;