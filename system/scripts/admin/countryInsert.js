Vue.component('countryInsert', {
    template: `
        <div class="position-fixed window-insert">
            <div class="bg-white p-4 container shadow-lg rounded">
                <div class="d-flex mb-3"><h5 class="ms-1 fw-bold flex-fill">Country</h5>
                    <button class="btn mt-0 pt-0 px-0" @click="close">
                        <i class="bi bi-x text-danger"></i>
                    </button>
                </div>
                <div class="d-flex mb-3">
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Id</label>
                        <input class="form-control" v-model="item.id" placeholder="Id">
                    </div>
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Label</label>
                        <input class="form-control" v-model="item.label">
                    </div>
                    <div class="position-relative flex-fill">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Short</label>
                        <input class="form-control" v-model="item.short">
                    </div>
                </div>
                <div class="input-group">
                    <button class="btn btn-primary form-control" v-on:click='insertOrUpdate'>Insert or Update</button>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            item: this.sourceItem,
        }
    },
    props: {
        sourceItem: {
            type: Object
        },
    },
    methods: {
        insertOrUpdate: async function () {
            const insertUrl = "../be/countries/insert_countries";
            const labelsUrl = "../be/items/get_labels";
            var data = {
                method: "InsertOrUpdateCountry",
                country: this.item
            }
            await this.getItemsFromDb(insertUrl, data);

            var labels = await this.getItemsFromDb(labelsUrl, {
                method: "GetAllLabels"
            });
            Vue.prototype.$labels = labels;
            this.$labels = labels;
            this.$emit("itemInserted");
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
        close: async function () {
            const labelsUrl = "../be/items/get_labels";
            var labels = await this.getItemsFromDb(labelsUrl, {
                method: "GetAllLabels"
            });
            Vue.prototype.$labels = labels;
            this.$labels = labels;
            this.$emit("itemInserted");
        }
    },
});

