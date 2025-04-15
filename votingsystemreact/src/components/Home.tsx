import React from 'react';
import { Container, Typography, Box, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';

const Home = () => {
    const navigate = useNavigate();

    return (
        <Container component="main"
            maxWidth={false}
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                minHeight: '100vh',
                maxWidth: '100%',
                textAlign: 'center' }}>
            <Box sx={{ marginBottom: '20px' }}>
                <Typography variant="h3" component="h1" gutterBottom>
                    Welcome to the Voting System
                </Typography>
                <Typography variant="h6" component="p">
                    After logging in, you can see the active votes and participate in polls.
                </Typography>
            </Box>
            <Box sx={{ display: 'flex', justifyContent: 'center', gap: '20px' }}>
                <Button variant="contained" color="primary" onClick={() => navigate('/login')}>
                    Login
                </Button>
                <Button variant="outlined" color="primary" onClick={() => navigate('/register')}>
                    Register
                </Button>
            </Box>
        </Container>
    );
};

export default Home;