import { useState } from 'react';
import { useNavigate } from 'react-router-dom'; // ← ВАЖНО: добавить этот импорт
import { useStores } from '../context/RootStoreContext';
import { Button, TextInput, PasswordInput, Paper, Title, Container } from '@mantine/core';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const { auth } = useStores();
    const navigate = useNavigate(); // ← теперь работает

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await auth.login(email, password);
        if (auth.isAuthenticated) {
            navigate('/');
        }
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