type Email = string
type Phone = string

type User1 = {
  name: string
  password: string
  email: Email | null
  isEmailVerified: boolean
  canLogin: boolean
}

type UnVerifiedUser = {
  name: string
  password: string
}

type VerifiedEmailUser = {
  name: string
  password: string
  email: Email
}

type User =
  | UnVerifiedUser
  | VerifiedEmailUser
