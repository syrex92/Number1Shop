import { observer } from 'mobx-react-lite';
import { useNavigate } from 'react-router-dom';
import { Container } from '@mantine/core';
import { useStores } from "../context/RootStoreContext.tsx";
import LoginForm from '../components/auth/LoginForm.tsx';
import LoginHeader from '../components/auth/LoginHeader.tsx';
import ReturnButton from '../components/auth/ReturnButton.tsx';

const LoginPage = observer(() => {
  const { auth } = useStores();
  const navigate = useNavigate();

  
  // Если пользователь уже аутентифицирован, перенаправляем на главную
  if (auth.isAuthenticated) {
    navigate('/');
    return null;
  }

  return (
    <Container size={420} my={40}>
      <LoginHeader />
      <LoginForm />
      <ReturnButton/>
    </Container>
  );
});

export default LoginPage;