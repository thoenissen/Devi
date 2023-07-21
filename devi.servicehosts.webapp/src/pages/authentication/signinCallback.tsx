// React
import * as React from 'react'
import { useNavigate } from 'react-router-dom'

// Services
import { useAuth } from '../../provider/authentication/authenticationProvider'

const SigninCallback: React.FC = () => {
    const { onSigninCallback } = useAuth()
    const navigate = useNavigate()

    React.useEffect(() => {
        ;(async () => {
            try {
                onSigninCallback?.()
            } catch (e) {
                console.error(e)
            } finally {
                navigate('/')
            }
        })()
    })

    return <h1>Signing in...</h1>
}

export default SigninCallback
