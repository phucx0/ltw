import { SePayPgClient } from 'sepay-pg-node';

const merchantId = 'SP-TEST-PH453939';
const secret_key = "spsk_test_YNehJMyy5y3hsZFHB8uEjE11VA2ajbxb";
const client = new SePayPgClient({
    env: 'sandbox',
    merchant_id: merchantId,
    secret_key: secret_key
});

const checkoutURL = client.checkout.initCheckoutUrl();

const checkoutFormfields = client.checkout.initOneTimePaymentFields({
    payment_method: 'BANK_TRANSFER',
    order_invoice_number: 'DH123',
    order_amount: 10000,
    currency: 'VND',
    order_description: 'Thanh toan don hang DH123',
    success_url: 'https://example.com/order/DH123?payment=success',
    error_url: 'https://example.com/order/DH123?payment=error',
    cancel_url: 'https://example.com/order/DH123?payment=cancel',
});

return (
    <form action={checkoutURL} method="POST">
        {Object.keys(checkoutFormfields).map(field => (
            <input type="hidden" name={field} value={checkoutFormfields[field]} />
        ))}
        <button type="submit">Pay now</button>
    </form>
);