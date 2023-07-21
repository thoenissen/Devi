// React
import React, { createContext, useContext, ReactNode, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'

// Services
import AuthenticationService from '../../services/authentication/authenticationService'

interface AuthenticationContextType {
    isLoggedIn: boolean
    onLogin: (() => Promise<void>) | null
    onLogout: (() => void) | null
    onSigninCallback: (() => void) | null
    onSilentSignin: (() => void) | null
}

export const AuthContext = createContext<AuthenticationContextType>({
    isLoggedIn: false,
    onLogin: null,
    onLogout: null,
    onSigninCallback: null,
    onSilentSignin: null
})

type AuthenticationContextProviderProps = {
    children?: ReactNode
}

export const AuthProvider = ({ children }: AuthenticationContextProviderProps) => {
    const navigate = useNavigate()
    const location = useLocation()
    const service = new AuthenticationService()
    const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false)

    const login = async () => {
        service.signin()
    }

    const logout = () => {
        setIsLoggedIn(false)

        service.signout()
    }

    const signin = () => {
        service.signinCallback()

        const user = service.getUser()

        if (user != null) {
            setIsLoggedIn(true)

            const origin = location.state?.from?.pathname || '/'

            navigate(origin)
        }
    }

    const signinSilent = () => {
        service.signinSilent()
    }

    const value = {
        isLoggedIn: isLoggedIn,
        onLogin: login,
        onLogout: logout,
        onSigninCallback: signin,
        onSilentSignin: signinSilent
    }

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => useContext(AuthContext)
