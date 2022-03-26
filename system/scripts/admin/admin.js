Vue.component('admin', {
    template: `
        <div class="container">
            <a class="btn btn-primary" href="/admin/items">Add items</a>
            <button class="btn btn-primary" @click="updatePrices" :disabled="isUpdateingPrices">Get prices</button>
        </div>
    `,
    data() {
        return {
            isUpdateingPrices: false
        }
    },
    methods: {
        updatePrices: async function () {
            const url = "../be/prices/update_prices";
            var data = {};
            this.isUpdateingPrices = true;
            await this.getItemsFromDb(url, data);
            this.isUpdateingPrices = false;
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
    },
});

