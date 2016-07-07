/**
 * Created by Excelle on 7/7/16.
 */

$(document).ready(function () {
    var args = getArgs();
    if (!args.customer || !args.room) {
        location.assign('/');
    }

    getApi('/api/customer/' + args.customer, {}, function (err, r) {
        if (err) {
            alert('Error when loading customer - ' + err.message);
        } else {
            deleteApi('/api/room/' + args.room + '/customer', r,
                function (err2, c) {
                    if (err2) {
                        alert('Error when loading customer: ' + err2.message);
                    } else {
                        getApi('/api/consumption/' + args.customer, {},
                        function (err3, cs) {
                            if (err3) {
                                alert('Error when loading consumption record: ' + err3.message);
                            } else {
                                getApi('/api/room/' + args.room, {}, function (err4, rm) {
                                    if (err4) {
                                        alert('Error loading room - ' + err4.message);
                                    } else {
                                        checkoutView(c, rm, cs);
                                    }
                                })
                            }
                        })
                    }
                });
        }
    })
});


function checkoutView(customer, room, consumptions) {
    var vm = new Vue({
        el: '#checkout-form',
        data: {
            customer: customer,
            room: room,
            consumptions: consumptions
        },
        methods: {
            getSum: function () {
                var sum = 0.0;
                for(var i in this.consumptions) {
                    sum += this.consumptions[i].price;
                }
                return sum;
            },

            submit: function () {
                deleteApi('/api/consumption/' + this.customer.id, {}, function (err, r) {
                    if (err) {
                        alert('提交时发生错误: ' + err.message);
                    } else {
                        alert('结算成功!');
                        location.assign('/');
                    }
                });
            }
        }
    });
}