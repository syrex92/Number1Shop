import { observer } from 'mobx-react-lite';
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useStores } from '../context/RootStoreContext';
import { Button, TextInput, PasswordInput, Paper, Title, Container } from '@mantine/core';

const LoginPage = observer(() => {
    const [name, setName] = useState('');
    const [password, setPassword] = useState('');
    const { auth } = useStores();
    const navigate = useNavigate();

    useEffect(() => {
        console.log('LoginPage effect, isAuthenticated =', auth.isAuthenticated);
        if (auth.isAuthenticated) {
            console.log('LoginPage: Redirecting to /');
            navigate('/');
        }
    }, [auth.isAuthenticated, navigate]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        console.log('LoginPage: submitting', name);
        await auth.login(name, password);
        console.log('LoginPage: after login, isAuthenticated =', auth.isAuthenticated);
    };

    return (
        <Container size={420} my={40}>
            <Title ta="center">Добро пожаловать</Title>
            <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                <form onSubmit={handleSubmit}>
                    <TextInput
                        label="Name"
                        placeholder="user"
                        required
                        value={name}
                        onChange={(e) => setName(e.target.value)}
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
});

export default LoginPage;