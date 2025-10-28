
import {Routes, Route, Navigate} from 'react-router-dom';
import './App.css';
import {useStores} from './context/RootStoreContext';
import LoginPage from './pages/LoginPage';
import MainLayout from './pages/MainLayout';
import '@mantine/core/styles.css';

import {MantineProvider} from '@mantine/core';

function App() {
    const {auth} = useStores();

    return (
        <MantineProvider defaultColorScheme="auto">        
            <Routes>
                <Route path="/login" element={auth.isAuthenticated ? <Navigate to="/" replace/> : <LoginPage/>}/>
                <Route path="/*" element={<MainLayout/>}/>
            </Routes>
        </MantineProvider>
    );
}

export default App;
