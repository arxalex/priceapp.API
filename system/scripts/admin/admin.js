Vue.component('admin', {
    template: `
        <div class="container">
            <a class="btn btn-primary" href="/admin/items">Add items</a>
            <button class="btn btn-primary" @click="updatePrices" :disabled="isUpdateingPrices">Get prices</button>
            <button class="btn btn-primary" @click="updateFilials" :disabled="isUpdateingFilials">Get filials</button>
        </div>
    `,
    data() {
        return {
            isUpdateingPrices: false,
            isUpdateingFilials: false
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
        updateFilials: async function () {
            const url = "../be/filials/get_filials";
            var data = {};
            this.isUpdateingFilials = true;
            await this.getItemsFromDb(url, data);
            this.isUpdateingFilials = false;
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

