
import { Routes, Route } from 'react-router-dom';
import './App.css';
import LoginPage from './pages/LoginPage';
import MainLayout from './pages/MainLayout';
import '@mantine/core/styles.css';

import { MantineProvider } from '@mantine/core';
import RegistrationPage from './pages/RegistrationPage';

function App() {
    return (
        <MantineProvider defaultColorScheme="dark">
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/registration" element={<RegistrationPage />} />
                <Route path="/" element={<MainLayout />} />
                <Route path="*" element={<MainLayout />} />
            </Routes>
        </MantineProvider>
    );
}

export default App;
