// React
import React from 'react'
import { Route, Routes, BrowserRouter } from 'react-router-dom'

// Pages
import HomePage from './pages/general/homePage'
import SigninCallback from './pages/authentication/signinCallback'
import SigninRenew from './pages/authentication/signinRenew'
import NotFoundPage from './pages/general/notFoundPage'

// Styles
import './styles/main.scss'

// Services
import { AuthProvider } from './provider/authentication/authenticationProvider'

const RootComponent: React.FC = () => {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Routes>
                    {/* Home page */}
                    <Route path="/" element={<HomePage />} />

                    {/* Authentication */}
                    {/*<Route path="/login" element={<LoginPage/>} />*/}
                    <Route path="/signin" element={<SigninCallback />} />
                    <Route path="/signin-renew" element={<SigninRenew />} />

                    {/* 404 page */}
                    <Route path="*" element={<NotFoundPage />} />
                </Routes>
            </AuthProvider>
        </BrowserRouter>
    )
}

export default RootComponent
