import { Title, Text, Anchor } from '@mantine/core';
import { Link } from 'react-router-dom';

const LoginHeader = () => {
    return (
        <>
            <Title ta="center" m={10}>
                Добро пожаловать!
            </Title>

            <Text c="dimmed" size="sm" ta="center">
                Еще нет аккаунта? 
                <Anchor size="sm" p={20} component={Link} to="/registration">
                    Зарегистрироваться
                </Anchor>
            </Text>
        </>
    );
};

export default LoginHeader;