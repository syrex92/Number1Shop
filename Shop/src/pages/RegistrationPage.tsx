import { observer } from 'mobx-react-lite';
import { useNavigate } from 'react-router-dom';
import { Container } from '@mantine/core';
import { useStores } from "../context/RootStoreContext.tsx";
import ReturnButton from '../components/auth/ReturnButton.tsx';
import RegistrationHeader from '../components/registration/RegistrationHeader.tsx';
import RegistrationForm from '../components/registration/RegistrationForm.tsx';

const RegistrationPage = observer(() => {
  const { auth } = useStores();
  const navigate = useNavigate();

  // Если пользователь уже аутентифицирован, перенаправляем на главную
  if (auth.isAuthenticated) {
    navigate('/');
    return null;
  }

  return (
      <Container size={600} my={40}>
      <RegistrationHeader/>
      <RegistrationForm />
      <ReturnButton/>
    </Container>
  );
});

export default RegistrationPage;