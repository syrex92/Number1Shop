import { Title, Text, Anchor } from '@mantine/core';
import { Link } from 'react-router-dom';

const RegistrationHeader = () => {
    return (
        <>
            <Title ta="center" m={10}>
                Форма регистрации
            </Title>
            <Text c="dimmed" size="sm" ta="center">
                Уже есть аккаунт?{' '}
                <Anchor size="sm" p={20} component={Link} to="/login">
                    Войти
                </Anchor>
            </Text>
        </>
    );
};

export default RegistrationHeader;