import { Title, Text, Anchor } from '@mantine/core';
import { useNavigate } from 'react-router-dom';

const LoginHeader = () => {
    const navigate = useNavigate();

    const handleRegister = () => {
        navigate("/");
    }
    return (
        <>
            <Title ta="center" m={10}>
                Добро пожаловать!
            </Title>

            <Text c="dimmed" size="sm" ta="center">
                Еще нет аккаунта? 
                <Anchor size="sm" p={20}  onClick={handleRegister}>
                    Зарегистрироваться
                </Anchor>
            </Text>
        </>
    );
};

export default LoginHeader;