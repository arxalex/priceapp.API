Vue.component('admin', {
    template: `
        <div class="container">
            <a class="btn btn-primary" href="/admin/items">Add items</a>
            <button class="btn btn-primary" @click="updatePrices" :disabled="isUpdateingPrices">Get prices</button>
            <button class="btn btn-primary" @click="updateFilials" :disabled="isUpdateingFilials">Get filials</button>
            <button class="btn btn-primary" @click="analizePrices" :disabled="isAnalizingPrices">Analize and refactor prices</button>
            <button class="btn btn-primary" @click="getAndInserCategories" :disabled="isInsertingCategories">Get categories from shops and insert to db</button>
        </div>
    `,
    data() {
        return {
            isUpdateingPrices: false,
            isUpdateingFilials: false,
            isAnalizingPrices: false,
            isInsertingCategories: false
        }
    },
    methods: {
        updatePrices: async function () {
            const url = "../be/prices/update_prices";
            var end = false;
            var page = 0;
            while (!end) {
                var data = {
                    from: page * 5 + 1,
                    to: page * 5 + 5
                };
                page++;
                this.isUpdateingPrices = true;
                var status = await this.getItemsFromDb(url, data);
                end = !status.statusUpdate;
            }
            this.isUpdateingPrices = false;
        },
        updateFilials: async function () {
            const url = "../be/filials/get_filials";
            for (var i = 1; i <= 3; i++) {
                var data = {
                    source: i,
                    shopid: i
                };
                this.isUpdateingFilials = true;
                await this.getItemsFromDb(url, data);
                this.isUpdateingFilials = false;
            }
        },
        getAndInserCategories: async function () {
            const url = "../be/categories/get_and_insert_categories_to_db";
            for (var i = 1; i <= 3; i++) {
                var data = {
                    source: i,
                    method: "GetCategoryFromSourceAndInsert"
                };
                this.isInsertingCategories = true;
                await this.getItemsFromDb(url, data);
                this.isInsertingCategories = false;
            }
        },
        analizePrices: async function () {
            const url = "../be/prices/analize_prices";
            var data = {
                method: "fix10x"
            };
            this.isAnalizingPrices = true;
            await this.getItemsFromDb(url, data);
            this.isAnalizingPrices = false;
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

