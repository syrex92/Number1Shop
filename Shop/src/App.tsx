import {Routes, Route} from 'react-router-dom';
import './App.css';
import LoginPage from './pages/LoginPage';
import MainLayout from './pages/MainLayout';
import '@mantine/core/styles.css';
import '@mantine/carousel/styles.css';

import {MantineProvider} from '@mantine/core';
import RegistrationPage from './pages/RegistrationPage';
import {ModalsProvider} from "@mantine/modals";

function App() {
    return (
        <MantineProvider defaultColorScheme="dark">
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
