// type key word
type Name1 = {
  firstName: string
  middleName: string
  lastName: string
}

// type alias
const time = 10

type Second = number
const timeS: Second = 10

// Union type
type Fish = string
type Bird = string

type Pet = Fish | Bird

// Discriminating Unions
type Alipay = {
  state: "alipay"
}

type Cash = {
  state: "cash"
}

type CreditCard0 = {
  state: "creditCard"
  cardNo: string
}

type PaymentMethod =
  | Alipay
  | Cash
  | CreditCard

type Order = {}

// 函数声明
type PayOrder = (amount: number) => (payment: PaymentMethod) => Order
type Add = (a: number) => (b: number) => number
const add: Add = a => b => a + b

// Option
type Option<T> =  T | null

type Customer = {
  FirstName: string
  LastName: string
  Address: string
  EmailAddress: string
  PhoneNumber: string
  IsPhoneVerified: boolean
}

// optional
// length
// Atomic
// When verified is true or false


type CreditCard = {
  cardNo: string
  firstName: string
  middleName: string
  lastName: string
  isExpired: boolean
}

type CreditCard1 = {
  cardNo: string
  firstName: string
  middleName: Option<string>
  lastName: string
  isExpired: boolean
}

type CardNo = string
type Name50 = string

type CreditCard2 = {
  cardNo: Option<CardNo>
  firstName: Name50
  middleName: Option<string>
  lastName: Name50
  contactEmail: Email
  contactPhone: Phone
}

type Email = string
type Phone = string

type GetCardNo = (cardNo: string) => Option<CardNo>

type Name = {
  firstName: Name50
  middleName: Option<string>
  lastName: Name50
}

type Contact = {
  contactEmail: Email
  contactPhone: Phone
}

type CreditCard3 = {
  cardNo: Option<CardNo>
  name: Name
  contact: Contact
}


type Contact1 = {
  contactEmail: Option<Email>
  contactPhone: Option<Phone>
}

type OnlyContactEmail = Email
type OnlyContactPhone = Phone
type BothContactEmailAndPhone = Email & Phone

type Contact2 =
  | OnlyContactEmail
  | OnlyContactPhone
  | BothContactEmailAndPhone


