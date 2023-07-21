import { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from './provider/authentication/authenticationProvider'

type ProtectedRouteProps = {
    children?: ReactNode
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
    const { isLoggedIn } = useAuth()
    const location = useLocation()

    if (isLoggedIn == false) {
        return <Navigate to="/login" replace state={{ from: location }} />
    }

    return children
}
