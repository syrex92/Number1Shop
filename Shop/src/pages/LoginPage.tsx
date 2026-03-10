import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useStores } from '../context/RootStoreContext';
import { Button, TextInput, PasswordInput, Paper, Title, Container } from '@mantine/core';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const { auth } = useStores();
    const navigate = useNavigate();

    useEffect(() => {
        console.log('LoginPage render, isAuthenticated =', auth.isAuthenticated);
        console.log('LoginPage render, accessToken =', auth.accessToken);
        if (auth.isAuthenticated) {
            console.log('LoginPage: Redirecting to /');
            navigate('/');
        }
    }, [auth.isAuthenticated, navigate]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        console.log('LoginPage: submitting', email);
        await auth.login(email, password);
        console.log('LoginPage: after login, isAuthenticated =', auth.isAuthenticated);
    };

    return (
        <Container size={420} my={40}>
            <Title ta="center">Добро пожаловать</Title>
            <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                <form onSubmit={handleSubmit}>
                    <TextInput
                        label="Email"
                        placeholder="your@email.com"
                        required
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    <PasswordInput
                        label="Пароль"
                        placeholder="Ваш пароль"
                        required
                        mt="md"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    {auth.error && (
                        <p style={{ color: 'red', marginTop: 10 }}>{auth.error}</p>
                    )}
                    <Button type="submit" fullWidth mt="xl" loading={auth.isLoading}>
                        Войти
                    </Button>
                </form>
            </Paper>
        </Container>
    );
}

export default LoginPage;