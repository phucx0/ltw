const connection = new signalR.HubConnectionBuilder()
    .withUrl("/paymentHub")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

connection.on("PaymentConfirmed", data => {
    console.log(data);
    if (data.success) {
        console.log("Thanh toán thành công cho đơn:", data.bookingId);
        window.location.href = `/Booking/Payment/Success?bookingId=${data.bookingId}`;
    } else {
        window.location.href = `/Booking/Payment/Failed?bookingId=${data.bookingId}`;
    }
});

const container = document.querySelector('.checkout-container');
const userId = container.dataset.userId;
console.log(userId)
connection.start()
    .then(() => {
        console.log("SignalR Connected - ConnectionId:", connection.connectionId);

        // Gửi userId lên server để join vào group riêng
        connection.invoke("RegisterUser", userId)
            .then(() => {
                console.log("Đã đăng ký user:", userId);
            })
            .catch(err => {
                console.error("Lỗi RegisterUser:", err);
            });
    })
    .catch(err => console.error("SignalR Connection Error:", err));