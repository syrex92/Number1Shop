import { useState } from 'react';
import type { FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import '../styles/LoginPage.css';

const LoginPage = observer(() => {
  const { auth } = useStores();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const onSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    await auth.login(email, password);
    if (auth.isAuthenticated) navigate('/');
  };

  return (
    <div className="login-page">
      <form className="login-form" onSubmit={onSubmit}>
        <h2>Вход</h2>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Пароль"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit" disabled={auth.isLoading}>Войти</button>
        {auth.error && <div className="error">{auth.error}</div>}
      </form>
    </div>
  );
});

export default LoginPage;